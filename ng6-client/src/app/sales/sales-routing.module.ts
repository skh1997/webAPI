import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SalesAppComponent } from './sales-app.component';
import { ProductComponent } from './components/product/product.component';

const routes: Routes = [
  {
    path: '', component: SalesAppComponent,
    children: [
      { path: 'product', component: ProductComponent },
      { path: '**', redirectTo: 'product' }
    ]
  },
  { path: '**', redirectTo: '' }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SalesRoutingModule { }
